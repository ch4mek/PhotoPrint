export interface ChartDataItem {
	name: { date: number; weekDay: string } | string;
	hours: number;
	minutes: number;
}

export interface ChartProps {
	data: ChartDataItem[];
	showLoader?: boolean;
	xAxisLabel?: string;
}

export type BarBorders = [number, number, number, number];
