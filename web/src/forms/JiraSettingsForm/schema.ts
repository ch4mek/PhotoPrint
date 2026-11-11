import * as yup from 'yup';

import { textType4Schema } from '../schemas';
import { ddlType1Schema } from '../schemas/types/ddlType1';
import { emailType2Schema } from '../schemas/types/emailType2';
import { linkSchema } from '../schemas/types/linkType';

import { FIELD_IS_REQUIRED } from '@/constants/errors';

const jiraSettingsBaseSchema = {
	url: linkSchema(true),
	email: emailType2Schema().required(FIELD_IS_REQUIRED),
	projectTag: textType4Schema('project tag'),
	project: ddlType1Schema(),
};

export const getJiraSettingsFormSchema = (isEditing: boolean) =>
	yup.object({
		...jiraSettingsBaseSchema,
		personalAccessToken: textType4Schema('PAT', !isEditing),
	});
