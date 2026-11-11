import { FormUserAutoselectOption } from '@/types/forms';
import { UseUsersResponse } from '@/types/userInfo';

export enum AddGroupMemberFormFields {
	USER = 'user',
}

export type AddGroupMemberFormValues = {
	[AddGroupMemberFormFields.USER]: FormUserAutoselectOption | null;
};

export type AddGroupMemberFormProps = {
	onClose: VoidFunction;
	options: {
		[AddGroupMemberFormFields.USER]: Array<UseUsersResponse>;
	};
};
