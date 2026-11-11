export interface InputTimeCellProps {
	id: number;
	time: string | null;
	hasError: boolean;
	rowIdx: number;
	monthId: number;
	countryId: number;
	disabled?: boolean;
	shouldDisplayTooltip: boolean;
}
