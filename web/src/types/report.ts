export interface ReportResponseItem {
	userId: number;
	lastName: string;
	firstName: string;
	totalTimeInMinutes: number;
	projectsCount: number;
	color: string;
}

export interface NavigatorDate {
	year: number;
	month?: string;
	quarter?: string;
}
