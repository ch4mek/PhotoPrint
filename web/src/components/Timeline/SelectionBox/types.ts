import { SelectedTimePeriod } from '../types';

export interface SelectionBoxProps {
	selectedPeriod: SelectedTimePeriod<number>;
	selectedSectionPositionIndex: number;
	selectedSectionId: number;
}
