import { PERSONAL_EMAILS_ISSUE } from '@/constants/errors';
import {
	EMAIL_DUPLICATE_MESSAGE_PATTERN,
	emailFormFields,
} from '@/constants/user';
import { AdminPanelUserAddingFormValues } from '@/forms/AdminPanelForms/UserAddingForm/types';
import { PersonalDataFormValues } from '@/forms/TeamboardForm/PersonalData/types';
import { EmailFieldName } from '@/types/personalData';

export const getEmailDuplicateFields = (
	message: string | undefined,
	values: AdminPanelUserAddingFormValues | PersonalDataFormValues,
	emailFields: EmailFieldName[]
): EmailFieldName[] => {
	const lowerMessage = message?.toLowerCase();
	const fields: EmailFieldName[] = [];

	if (lowerMessage?.includes(PERSONAL_EMAILS_ISSUE.personalAdditional)) {
		fields.push(emailFormFields.personalEmailSecond.name);
	}

	if (lowerMessage?.includes(PERSONAL_EMAILS_ISSUE.personalMain)) {
		fields.push(emailFormFields.personalEmail.name);
	}

	if (fields.length > 0) {
		return fields;
	}

	const duplicateEmail = message?.match(EMAIL_DUPLICATE_MESSAGE_PATTERN)?.[1];
	const matchedField = emailFields.find(
		(field) => values[field] === duplicateEmail
	);

	return matchedField ? [matchedField] : [];
};
