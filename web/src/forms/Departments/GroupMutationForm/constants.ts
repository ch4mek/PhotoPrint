import { GroupMutationFormValues } from './types';

export const groupMutationFormFieldsNames: Record<
	keyof GroupMutationFormValues,
	string
> = {
	name: 'name',
	groupManager: 'groupManager',
};

export const groupMutationFormFieldsLabels: Record<
	keyof GroupMutationFormValues,
	string
> = {
	name: 'Group name',
	groupManager: 'Group manager',
};

export const FORM_NAME = 'Group mutation form';
