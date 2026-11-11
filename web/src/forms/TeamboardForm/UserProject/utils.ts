import { ProjectAssignFormInitialValues } from '@/forms/ProjectAccessAddingBase/types';
import { findById } from '@/helpers/findById';
import { findByName } from '@/helpers/findByName';
import dayjs from '@/lib/dayjs';
import { UserProjectAssignmentsByIdResponse } from '@/queries/types';
import { CalendaryCountry, SeniorityLevelType } from '@/types/configuration';
import { FormUserAutoselectOption, FormAutoselectOption } from '@/types/forms';

export const convertProjectAssignToInitialFormValues = (
	response: UserProjectAssignmentsByIdResponse,
	user: FormUserAutoselectOption,
	projects: Array<FormAutoselectOption>,
	seniorityLevels: Array<SeniorityLevelType>,
	countries: Array<CalendaryCountry>,
	isUserForm: boolean
): Required<ProjectAssignFormInitialValues> => {
	const {
		fte,
		userOnProjectFrom,
		userOnProjectTo,
		projectId,
		projectName,
		isTimeOnProjectBillable,
		seniorityLevel,
	} = response;

	return {
		billability: {
			value: isTimeOnProjectBillable,
		},
		fte: {
			value: String(fte),
		},
		startDate: {
			value: dayjs(userOnProjectFrom),
		},
		endDate: {
			value:
				userOnProjectTo !== null ? dayjs(userOnProjectTo) : userOnProjectTo,
		},
		user: {
			value: user,
			disabled: isUserForm,
		},
		project: {
			value: findById(projects, projectId) || {
				id: projectId,
				name: projectName,
			},
			disabled: !isUserForm,
		},
		projectSeniority: {
			value: findByName(seniorityLevels, seniorityLevel) || {
				id: seniorityLevel,
				name: seniorityLevel,
			},
		},
	};
};
