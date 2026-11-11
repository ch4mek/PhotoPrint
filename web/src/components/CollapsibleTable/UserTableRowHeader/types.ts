export interface UserTableRowHeaderProps {
	totalTrackedInMinutes: number;
	countOfProjects: number;
	userName: string;
	color: string;
	handleSetRowOpen: () => void;
	isRowOpen: boolean;
	isDisplayFact?: boolean;
}
