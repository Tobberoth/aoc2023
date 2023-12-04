use std::io::prelude::*;

const INPUT_LINES: usize = 223;

fn main() {
    let f = std::fs::File::open("input.txt").expect("Unable to open input file");
    let lines = std::io::BufReader::new(f).lines().map(|l| l.unwrap()).collect::<Vec<_>>();
    let mut card_amounts = [1; INPUT_LINES];
    let mut sum: i32 = 0;
    for (current_line_index, line) in lines.iter().enumerate() {
        let numbers = line.split_once(':').unwrap().1;

        let winning_set = numbers.split('|').next().unwrap()
            .split(' ').filter_map(|n| n.parse().ok()).collect::<std::collections::HashSet<i32>>();
        let card_numbers_set = numbers.split('|').nth(1).unwrap()
            .split(' ').filter_map(|n| n.parse().ok()).collect::<std::collections::HashSet<i32>>();
        let matches = winning_set.intersection(&card_numbers_set).count() as u32;
        if matches > 0 {
            // Step 1
            let points = 2_i32.pow(matches - 1);
            sum += points;
            // Step 2
            for i in current_line_index+1..current_line_index+1+(matches as usize) {
                card_amounts[i] += card_amounts[current_line_index];
            }
        }
    }
    println!("{sum}");
    println!("{:?}", card_amounts.iter().sum::<u64>());
}
