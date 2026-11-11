export interface CustomizedXAxisTickProps {
	x?: number;
	y?: number;
	index?: number;
	values?: Array<{ name: { date: string; weekDay: string } }>;
	payload?: { value: string[] };
}
