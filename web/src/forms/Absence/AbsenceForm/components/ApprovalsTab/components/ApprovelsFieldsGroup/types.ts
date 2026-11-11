import { LookupUserResponse } from '@/types/userInfo';

export interface DynamicFieldsGroupProps {
	name: string;
	gridArea?: string;
	users?: LookupUserResponse[] | undefined;
	maxNumberOfFields?: number;
}
