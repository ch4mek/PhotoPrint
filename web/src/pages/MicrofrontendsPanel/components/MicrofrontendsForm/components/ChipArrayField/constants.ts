export const CHIP_ARRAY_FIELD_ERRORS = {
	EMPTY_VALUE: 'Value cannot be empty',
	DUPLICATE_VALUE: 'This value already exists',
	MAX_ITEMS_REACHED: (max: number) => `Maximum ${max} items allowed`,
} as const;

export const CHIP_ARRAY_FIELD_LABELS = {
	ADD: 'Add item',
	MAX_REACHED: (max: number) => `Maximum ${max} items reached`,
} as const;
