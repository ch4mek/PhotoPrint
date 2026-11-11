import { PieChartResponseFull } from '@/types/pieChart';

export interface EmployeeListProps {
	chartData?: {
		[key: string]: PieChartResponseFull[];
	};

	generalDuration: {
		[key: string]: string;
	};
}
