export const INITIAL_ORDER_BY_VALUE = 'id';

export const NUMERAL_PRICE_FORMAT = '0,0.00';

export const AssignStatus = {
	Assigned: 'assigned',
	Returned: 'returned',
} as const;

export const assignStatusLabel = {
	[AssignStatus.Assigned]: 'Assigned',
	[AssignStatus.Returned]: 'Returned',
} as const;
