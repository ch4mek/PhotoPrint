export interface ChartStackedProjectItemResponse {
	projectId: string;
	projectName: string;
	totalTime: number;
	projectColor: string;
	isBillableProject: boolean;
}

export interface ChartStackedProjectResponse {
	period: string;
	totalTimeInMinutes: number;
	projects: ChartStackedProjectItemResponse[];
}

export interface ChartStackedResponse {
	totalTimeInMinutes: number;
	timePerPeriod: ChartStackedProjectResponse[];
	projects: { projectId: number }[];
}

interface ChartStackedProjectItemAdapted
	extends ChartStackedProjectItemResponse {
	hours: number;
	minutes: number;
	hoursForBar: number;
}

export interface ChartStackedProjectAdapted {
	name: { date: number; weekDay: string } | string;
	projects: ChartStackedProjectItemAdapted[];
	totalTimeInMinutes: number;
}

export interface ChartStackedProps {
	data: ChartStackedProjectAdapted[];
	showLoader: boolean;
	xAxisLabel?: string;
}

export interface TooltipPayload {
	value: [number, number];
	x: number;
	y: number;
	width: number;
	height: number;
	tooltipPayload: {
		dataKey: string;
		name: string;
		value: number;
	}[];
}

export type KeyColorItem = {
	projectId: string;
	projectName: string;
	projectColor: string;
};

export type BarBorders = [number, number, number, number];
