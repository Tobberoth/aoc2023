fn main() {
    let input = std::fs::read_to_string("input.txt").expect("Unable to read input.txt");
    let input = input.replace("\r", ""); // Remove windows newline if present
    let lines = input.split('\n').collect::<Vec<&str>>();
    println!("{}", step1(lines[0], lines[1]));
    println!("{}", step2(lines[0], lines[1]));
}

fn step1(time_string: &str, distance_string: &str) -> u64 {
    let times = time_string.split(' ').filter_map(|t| t.parse::<u64>().ok());
    let distances = distance_string.split(' ').filter_map(|t| t.parse::<u64>().ok());
    let races = times.zip(distances);
    races.map(|r| {
        let mut wins = 0;
        for i in 0..r.0 {
            if is_win(r.0, i, r.1) { wins += 1; }
        }
        wins
    }).product()
}

fn step2(time_string: &str, distance_string: &str) -> u64 {
    let time: u64 = time_string.split(' ').filter(|t| t.parse::<u64>().is_ok()).collect::<Vec<&str>>().join("").parse().unwrap();
    let distance: u64 = distance_string.split(' ').filter(|t| t.parse::<u64>().is_ok()).collect::<Vec<&str>>().join("").parse().unwrap();
    let mut wins = 0;
    for i in 0..time {
        if is_win(time, i, distance) { wins += 1; }
    }
    wins
}

fn is_win(time: u64, press: u64, target_distance: u64) -> bool {
    let time_remaining = time - press;
    let distance_traveled = time_remaining * press;
    distance_traveled > target_distance
}