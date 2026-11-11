import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import { getAdaptivePopUpContentStylesConfig } from '@/helpers/getAdaptivePopUpStylesConfig';
import { getFullName } from '@/helpers/utils';
import dayjs from '@/lib/dayjs';
import { FormAutoselectOption } from '@/types/form';
import { AddProjectExitPayload, ProjectExitEntity } from '@/types/projectExits';
import { LookupUserResponse } from '@/types/userInfo';

import {
	GetPopUpStylesConfigProps,
	ProjectExitFormValues,
	ValidatedProjectExitFormValues,
} from './types';

export const getPopUpStylesConfig = ({
	theme,
	windowWidth,
	windowHeight,
}: GetPopUpStylesConfigProps) => ({
	content: getAdaptivePopUpContentStylesConfig({
		theme,
		windowWidth,
		windowHeight,
		hasContols: true,
	}),
});

export const getUserOptions = (
	users: LookupUserResponse[] = []
): FormAutoselectOption[] =>
	users.map(({ id, firstName, lastName, firstNameRu, lastNameRu }) => ({
		id,
		name: getFullName(firstName, lastName, firstNameRu, lastNameRu),
	}));

export const getDefaultOptions = <T extends FormAutoselectOption>(
	values: T[] = []
): FormAutoselectOption[] => values.map(({ id, name }) => ({ id, name }));

export const convertDataToSubmit = (
	values: ValidatedProjectExitFormValues
): AddProjectExitPayload => ({
	userId: values.user.id,
	clientId: values.client.id,
	projectId: values.project.id,
	closingDate: values.closingDate.format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
	closingReasonName: values.closingReason.name,
});

export const getInitialValues = (
	projectExit: ProjectExitEntity
): ProjectExitFormValues => ({
	user: { id: projectExit.userId, name: projectExit.fullName },
	client: { id: projectExit.clientId, name: projectExit.clientName },
	project: { id: projectExit.projectId, name: projectExit.projectName },
	closingDate: dayjs(projectExit.closingDate),
	closingReason: {
		id: projectExit.closingReasonId,
		name: projectExit.closingReasonName,
	},
});
