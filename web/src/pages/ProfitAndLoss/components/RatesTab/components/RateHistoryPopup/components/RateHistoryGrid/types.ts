import { RateCellRow } from '@/pages/ProfitAndLoss/components/RatesTab/components/renderers/RateCellRenderer/types';

export interface RateHistoryGridRow extends RateCellRow {
	id: number;
	date: string;
	effectiveDate: string;
	rate: number;
	currency: string;
	handleEditClick?: (itemId?: string | number) => VoidFunction;
}

export interface RateHistoryGridProps {
	rows: RateHistoryGridRow[];
}
