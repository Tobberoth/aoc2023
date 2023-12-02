use regex::Regex;

const MAX_RED: i32 = 12;
const MAX_GREEN: i32 = 13;
const MAX_BLUE: i32 = 14;

fn main() {
    let input = std::fs::read_to_string("input.txt").expect("Unable to read file");
    let lines: Vec<&str> = input.split("\n").collect();

    let mut sum1 = 0;
    let mut sum2 = 0;

    for line in lines {
        let mut skip_id = false;
        let mut least_tup = (0, 0, 0); // Red, Green, Blue
        let id_re = Regex::new(r"Game (\d+):").unwrap();
        let id = id_re.captures(line).unwrap().get(1).unwrap().as_str();
        let amount_red = Regex::new(r"(\d+) red").unwrap();
        for cap in amount_red.captures_iter(line) {
            let amount: i32 = cap.get(1).unwrap().as_str().parse().expect("Unable to parse amount");
            if amount > MAX_RED { skip_id = true; }
            least_tup.0 = std::cmp::max(least_tup.0, amount);
        }
        let amount_green = Regex::new(r"(\d+) green").unwrap();
        for cap in amount_green.captures_iter(line) {
            let amount: i32 = cap.get(1).unwrap().as_str().parse().expect("Unable to parse amount");
            if amount > MAX_GREEN { skip_id = true; }
            least_tup.1 = std::cmp::max(least_tup.1, amount);
        }
        let amount_blue = Regex::new(r"(\d+) blue").unwrap();
        for cap in amount_blue.captures_iter(line) {
            let amount: i32 = cap.get(1).unwrap().as_str().parse().expect("Unable to parse amount");
            if amount > MAX_BLUE { skip_id = true; }
            least_tup.2 = std::cmp::max(least_tup.2, amount);
        }
        if !skip_id { sum1 += id.parse::<i32>().unwrap() }
        sum2 += least_tup.0 * least_tup.1 * least_tup.2;
    }

    println!("{sum1}");
    println!("{sum2}");
}
