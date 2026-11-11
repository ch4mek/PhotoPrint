export const getDefaultLabelText = (name: string, isDefault: boolean) =>
	`${name} ${isDefault ? '(Default)' : ''}`;

export const getLabelWithText = (
	stringToDisplay: string | number | null,
	amount?: number
) => `${amount} ${stringToDisplay}${amount === 1 ? '' : 's'}`;
