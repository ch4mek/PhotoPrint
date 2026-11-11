import { InputSizes } from '@mms/mms-ui-library/components/Input/types';

interface TextAreasBaseProps {
	name: string;
	label: string;
	size: keyof InputSizes;
	minRows: number;
	maxRows: number;
}

export interface TextAreas {
	comment: TextAreasBaseProps;
	plan: TextAreasBaseProps;
}
