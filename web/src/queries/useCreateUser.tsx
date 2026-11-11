import { Typography } from '@mms/mms-ui-library/components/Typography';
import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import {
	RESPONSE_EMAIL_EXIST,
	RESPONSE_USER_WITH_ROLE_EXISTS_START,
	RESPONSE_USER_WITH_ROLE_EXISTS_END,
} from '@/constants/errors';
import { NotificationType, USER_ADDED } from '@/constants/notifications';
import { GET_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/index';
import {
	checkIfMessageIsAboutEmailDuplicate,
	checkIfMessageIsAboutRoleDuplicate,
} from '@/helpers/checkIfMessageIsAboutDuplicate';
import { createDuplicateMessageRole } from '@/helpers/createDuplicateMessageRole';
import { createToast } from '@/helpers/createToast';
import { getDepartmentTitle } from '@/helpers/getDepartmentTitle';

import { CreateUserOptions, ServerResponseError } from './types';

export const useCreateUser = (
	options: UseMutationOptions<
		unknown,
		AxiosError<ServerResponseError>,
		CreateUserOptions,
		unknown
	>
) => {
	const toast = useToast();
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	return useMutation<
		unknown,
		AxiosError<ServerResponseError>,
		CreateUserOptions
	>({
		...options,
		mutationFn: async (userData) => {
			const { data } = await axios.post(GET_USERS_API_ENDPOINT, userData);

			return data;
		},
		onSuccess(data, variables, context) {
			toast.open(createToast(NotificationType.SUCCESS, USER_ADDED));

			if (options.onSuccess) {
				options.onSuccess(data, variables, context);
			}
		},

		onError(error, variables, context) {
			const { position, departmentId } = variables;
			const department =
				departmentTypes.find(({ id }) => id === departmentId) ?? null;
			const departmentTitle = getDepartmentTitle(position, department);

			const errorMessage = error.response?.data?.Message?.trim();

			if (checkIfMessageIsAboutEmailDuplicate(errorMessage)) {
				toast.open(
					createToast(
						NotificationType.ERROR,
						errorMessage || RESPONSE_EMAIL_EXIST
					)
				);

				return;
			}

			if (checkIfMessageIsAboutRoleDuplicate(errorMessage)) {
				toast.open(
					createToast(
						NotificationType.ERROR,
						<>
							{`${RESPONSE_USER_WITH_ROLE_EXISTS_START} `}
							<Typography variant="m-600">
								{createDuplicateMessageRole(
									position,
									departmentTitle,
									departmentTypes
								)}
							</Typography>
							{` ${RESPONSE_USER_WITH_ROLE_EXISTS_END}`}
						</>
					)
				);

				return;
			}

			if (options.onError) {
				options.onError(error, variables, context);
			}
		},
	});
};
