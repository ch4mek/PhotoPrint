export interface PieChartResponseFull {
	firstName: string;
	lastName: string;
	color: string;
	totalTimeInMinutes: number;
	name: string | null | undefined;
}

export interface ManagersPieChartResponse {
	firstName: string;
	lastName: string;
	color: string;
	trackedTime: number;
}
