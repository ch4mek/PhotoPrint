export type KPIColorsFormFields = 'minColor' | 'middleColor' | 'maxColor';

export type KPIColorFormSingleValue = {
	id: number;
	name: string;
	rightIntervalBorder: string;
};

export type KPIColorsFormValues = Record<
	KPIColorsFormFields,
	KPIColorFormSingleValue
>;
