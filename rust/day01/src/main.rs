fn main() {
    // Init
    let data = std::fs::read_to_string("input.txt").unwrap();
    let lines: Vec<&str> = data.split("\r\n").collect();

    // Step 1
    let mut ans: Vec<i32> = vec![];
    for line in lines.as_slice() {
        let a = find_first_last_number(line);
        ans.push(a.parse().unwrap());
    }
    let sum: i32 = ans.iter().sum();
    println!("{}", sum);

    // Step 2
    let mut sum2: u32 = 0;
    for line in lines.as_slice() {
        sum2 = sum2 + find_num(line)
    }
    println!("{}", sum2);
}

fn find_num(input: &str) -> u32 {
    let mut first: i32 = 1000;
    let mut first_ans: i32 = 0;
    let mut x: i32 = 1;
    while x < 10 {
        let index = find_first_of(input, x as u32);
        if index < first {
            first_ans = x;
            first = index;
        }
        x = x + 1;
    }
    let mut last: i32 = -1;
    let mut last_ans: i32 = 0;
    let mut y: i32 = 1;
    while y < 10 {
        let index = find_last_of(input, y as u32);
        if index > last {
            last_ans = y;
            last = index;
        }
        y = y + 1;
    }
    format!("{}{}", first_ans, last_ans).parse().unwrap()
}

fn find_first_of(input: &str, num: u32) -> i32 {
    let c_num = char::from_digit(num, 10).unwrap();
    let c_num_index = input.find(c_num).unwrap_or(1000);
    let s_num = match num {
        1 => "one",
        2 => "two",
        3 => "three",
        4 => "four",
        5 => "five",
        6 => "six",
        7 => "seven",
        8 => "eight",
        9 => "nine",
        _ => panic!("No match")
    };
    let s_num_index = input.find(s_num).unwrap_or(1000);
    std::cmp::min(c_num_index as i32, s_num_index as i32)
}

fn find_last_of(input: &str, num: u32) -> i32 {
    let c_num = char::from_digit(num, 10).unwrap();
    let c_num_index = match input.rfind(c_num) {
        Some(x) => x as i32,
        None => -1
    };
    let s_num = match num {
        1 => "one",
        2 => "two",
        3 => "three",
        4 => "four",
        5 => "five",
        6 => "six",
        7 => "seven",
        8 => "eight",
        9 => "nine",
        _ => panic!("No match")
    };
    let s_num_index = match input.rfind(s_num) {
        Some(x) => x as i32,
        None => -1
    };
    std::cmp::max(c_num_index as i32, s_num_index as i32)
}

fn find_first_last_number(input: &str) -> String {
    let pos = input.find(|c: char| c.is_digit(10)).unwrap();
    let pos2 = input.rfind(|c: char| c.is_digit(10)).unwrap();
    format!("{}{}", input.chars().nth(pos).unwrap(), input.chars().nth(pos2).unwrap())
}