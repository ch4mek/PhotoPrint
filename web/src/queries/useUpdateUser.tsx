import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import { Dispatch, SetStateAction } from 'react';

import {
	EDITING_CEO_USER_FORBIDDEN,
	EDITING_CEO_USER_RESPONSE,
	RESPONSE_EMAIL_EXIST,
	RESPONSE_USER_WITH_ROLE_EXISTS_END,
	RESPONSE_USER_WITH_ROLE_EXISTS_START,
} from '@/constants/errors';
import { NotificationType, USER_UPDATED } from '@/constants/notifications';
import { GET_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { Roles } from '@/constants/roles';
import { useConfiguration } from '@/context/Configuration';
import { useAuth, useToast } from '@/context/index';
import {
	checkIfMessageIsAboutEmailDuplicate,
	checkIfMessageIsAboutRoleDuplicate,
} from '@/helpers/checkIfMessageIsAboutDuplicate';
import { createDuplicateMessageRole } from '@/helpers/createDuplicateMessageRole';
import { createToast } from '@/helpers/createToast';
import { getDepartmentTitle } from '@/helpers/getDepartmentTitle';
import { UseUpdateUserParam, UseUpdateUserResponse } from '@/types/userInfo';

import { ServerResponseError } from './types';

export const useUpdateUser = (
	setIsEmailDuplicate: Dispatch<SetStateAction<boolean>>,
	options: UseMutationOptions<
		UseUpdateUserResponse,
		AxiosError<ServerResponseError>,
		UseUpdateUserParam
	>
) => {
	const toast = useToast();
	const { role } = useAuth();
	const isSysAdmin = role === Roles.SA;

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	return useMutation<
		UseUpdateUserResponse,
		AxiosError<ServerResponseError>,
		UseUpdateUserParam
	>({
		...options,
		mutationFn: async ({
			department,
			position,
			currentLocation,
			location,
			projects,
			firstName,
			lastName,
			firstNameRu,
			lastNameRu,
			patronymicRu,
			groupId,
			...otherValues
		}) => {
			const { data } = await axios.put(GET_USERS_API_ENDPOINT, {
				...otherValues,
				departmentId: department?.id ?? null,
				resourceGroupId: groupId ?? null,
				position: position.id,
				currentLocation: currentLocation || null,
				location: location?.name ?? null,
				isActive: false,
				firstName: isSysAdmin ? firstName : null,
				lastName: isSysAdmin ? lastName : null,
				firstNameRu: firstNameRu || null,
				lastNameRu: lastNameRu || null,
				patronymicRu: patronymicRu || null,
				projects: projects?.map((project) => ({
					projectId: project.id,
				})),
			});

			return data;
		},
		onSuccess(data, variables, context) {
			toast.open(createToast(NotificationType.SUCCESS, USER_UPDATED));

			if (options.onSuccess) {
				options.onSuccess(data, variables, context);
			}
		},

		onError(error, variables, context) {
			const { position, department } = variables;

			const positionTitle =
				typeof position === 'string' ? position : position.name;
			const departmentTitle = getDepartmentTitle(positionTitle, department);

			if (error?.response?.data?.Message) {
				const errorMessage = error?.response?.data?.Message?.trim();

				if (checkIfMessageIsAboutEmailDuplicate(errorMessage)) {
					setIsEmailDuplicate(true);
					toast.open(createToast(NotificationType.ERROR, RESPONSE_EMAIL_EXIST));
				}

				if (checkIfMessageIsAboutRoleDuplicate(errorMessage)) {
					toast.open(
						createToast(
							NotificationType.ERROR,
							<>
								{`${RESPONSE_USER_WITH_ROLE_EXISTS_START} `}
								<Typography variant="m-600">
									{createDuplicateMessageRole(
										positionTitle,
										departmentTitle,
										departmentTypes
									)}
								</Typography>
								{` ${RESPONSE_USER_WITH_ROLE_EXISTS_END}`}
							</>
						)
					);
				}

				if (errorMessage === EDITING_CEO_USER_RESPONSE) {
					toast.open(
						createToast(NotificationType.ERROR, EDITING_CEO_USER_FORBIDDEN)
					);
				}
			}

			if (options.onError) {
				options.onError(error, variables, context);
			}
		},
	});
};
