import { PieChartResponseFull } from '@/types/pieChart';

export interface EmployerListProps {
	chartData: {
		[key: string]: PieChartResponseFull[];
	};

	generalDuration: {
		[key: string]: string;
	};

	isFetching: boolean;
}

export interface ActiveChartIndexes {
	[chartType: string]: number | undefined;
}
