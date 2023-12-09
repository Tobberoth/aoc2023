fn main() {
    let data: String = std::fs::read_to_string("input.txt")
        .expect("Unable to read file")
        .replace("\r", "");
    let number_sequences = data.split('\n').map(|l| l.split(' ').filter_map(|n| n.parse::<i64>().ok()).collect::<Vec<i64>>());
    let sum: i64 = number_sequences.clone().map(|r| extrapolate_last(&r)).sum();
    println!("{sum}");
    let sum2: i64 = number_sequences.map(|r| extrapolate_first(&r)).sum();
    println!("{sum2}");
}

fn extrapolate_last(num_list: &Vec<i64>) -> i64 {
    let mut next_num_list = Vec::new();
    for i in 1..num_list.len() {
        next_num_list.push(num_list[i] - num_list[i-1]);
    }
    if next_num_list.iter().all(|n| *n == 0) {
        return num_list.last().unwrap().clone();
    }
    return num_list.last().unwrap() + extrapolate_last(&next_num_list);
}

fn extrapolate_first(num_list: &Vec<i64>) -> i64 {
    let mut next_num_list = Vec::new();
    for i in 1..num_list.len() {
        next_num_list.push(num_list[i] - num_list[i-1]);
    }
    if next_num_list.iter().all(|n| *n == 0) {
        return num_list.first().unwrap().clone();
    }
    return num_list.first().unwrap() - extrapolate_first(&next_num_list);
}