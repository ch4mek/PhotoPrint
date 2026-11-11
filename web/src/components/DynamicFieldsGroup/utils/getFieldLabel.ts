import { GetFieldLabelParams } from '../types';

export function getFieldLabel({
	index,
	label,
	mainLabel,
	additionalLabel,
}: GetFieldLabelParams): string {
	if (index === 0 && mainLabel) {
		return mainLabel;
	}

	if (index > 0 && additionalLabel) {
		return additionalLabel;
	}

	return label;
}
