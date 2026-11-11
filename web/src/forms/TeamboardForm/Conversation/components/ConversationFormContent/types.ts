import { Dispatch, SetStateAction } from 'react';

import {
	ConversationFormInitialValues,
	ConversationFormValues,
	DevelopmentPlanPopUp,
	UserSalaryPopUp,
} from '../../types';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { ConversationFormDocument } from '@/types/document';

export interface ConversationFormContentProps {
	formikProps: FormikWithBackupRenderProps<ConversationFormValues>;
	conversationPopUpTitle: string;
	isFetching: boolean;
	isLoading: boolean;
	isEditMode: boolean;
	initialValues: ConversationFormValues;
	initialConversationValues?: ConversationFormInitialValues;
	userSalaries: UserSalaryPopUp[];
	developmentPlans: DevelopmentPlanPopUp[];
	isHRAccess: boolean;
	isGroupManagerRole: boolean;
	onCloseConversationPopUp: () => void;
	onFileGroups: Dispatch<SetStateAction<ConversationFormDocument[]>>;
	onUserSalaries: Dispatch<SetStateAction<UserSalaryPopUp[]>>;
	onDevelopmentPlans: Dispatch<SetStateAction<DevelopmentPlanPopUp[]>>;
	id?: number;
	isIntern: boolean;
}
