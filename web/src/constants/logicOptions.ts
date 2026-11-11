import { SelectionModalOption } from '@/components/Grid/renderers/header/types';

export const logicOptions: Array<SelectionModalOption> = [
	{
		id: 'Yes',
		description: 'Yes',
		disabled: false,
	},
	{
		id: 'No',
		description: 'No',
		disabled: false,
	},
];

export const reversedLogicOptions: Array<SelectionModalOption> = [
	{
		id: 'No',
		description: 'Yes',
		disabled: false,
	},
	{
		id: 'Yes',
		description: 'No',
		disabled: false,
	},
];
