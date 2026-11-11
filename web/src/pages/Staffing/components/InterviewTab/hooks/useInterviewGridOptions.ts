import { useMemo } from 'react';

import { InterviewGridOptionsProps, InterviewOptions } from '../types';

import { getFullName } from '@/helpers/utils';

export const useInterviewGridOptions = ({
	users = [],
	managers = [],
}: InterviewGridOptionsProps) => {
	const options: InterviewOptions = useMemo(
		() => ({
			userOptions: users
				? users.map(({ firstName, lastName, firstNameRu, lastNameRu }) =>
						getFullName(firstName, lastName, firstNameRu, lastNameRu)
				  )
				: [],
			saleManagerOptions: managers
				? managers.map(({ firstName, firstNameRu, lastName, lastNameRu }) =>
						getFullName(firstName, lastName, firstNameRu, lastNameRu)
				  )
				: [],
		}),
		[users, managers]
	);

	return {
		defaultOptions: options,
	};
};
