export interface PayloadControlsBarProps {
	year: number;
	specializationOptions: string[];
	yearsOptions: string[];
	handleYearChange: (value: number | null) => void;
	handleSpecializationChange: (value: string | null) => void;
	specializationTitle: string;
	isMobileView: boolean;
}
