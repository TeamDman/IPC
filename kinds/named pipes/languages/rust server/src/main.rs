use std::time::Duration;
use tokio::io::AsyncWriteExt;
use tokio::net::windows::named_pipe::{ServerOptions, PipeMode};
use tokio::time;

const PIPE_NAME: &str = r"\\.\pipe\testpipe";

#[tokio::main]
async fn main() -> std::io::Result<()> {
    // Create the named pipe server
    let server = ServerOptions::new()
        .pipe_mode(PipeMode::Message) // Use message mode for discrete messages
        .first_pipe_instance(true) // First instance of the pipe server
        .create(PIPE_NAME)?;

    println!("Server started, waiting for client...");

    // Wait for the client to connect
    server.connect().await?;
    println!("Client connected!");

    // Write Fibonacci sequence to the client
    let mut writer = server;
    let mut a = 0;
    let mut b = 1;

    loop {
        let fib = a;
        a = b;
        b = a + fib;

        writer.write_all(format!("{}\n", fib).as_bytes()).await?;
        writer.flush().await?;
        
        // Wait before sending the next number
        time::sleep(Duration::from_secs(1)).await;
    }
}
