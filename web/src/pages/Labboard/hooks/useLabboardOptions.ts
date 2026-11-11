import { useMemo } from 'react';

import {
	LABBOARD_FINAL_RESULT_OPTIONS,
	LABBOARD_INTERNSHIP_TYPE_OPTIONS,
} from '../constants';
import { LabboardGridOptions } from '../types';

import { Departments } from '@/constants/departments';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { getFullName } from '@/helpers/utils';
import { useGroupsLookup } from '@/queries/groups/useGroupsLookup';
import { useLabboardUsersLookup } from '@/queries/labboard/useLabboardUsersLookup';

export const useLabboardOptions = () => {
	const {
		configurableValues: { englishLevels },
	} = useConfiguration();

	const { data: usersLookup } = useLabboardUsersLookup();
	const { data: groupsLookup } = useGroupsLookup(Departments.LD);

	const userOptions = useMemo(
		() =>
			(usersLookup || []).map(
				({ firstName, lastName, firstNameRu, lastNameRu }) =>
					getFullName(firstName, lastName, firstNameRu, lastNameRu)
			),
		[usersLookup]
	);

	const groupOptions = useMemo(
		() => [
			...(groupsLookup || []).map(({ name }) => name),
			NO_VALUE_FILTER_OPTION.name,
		],
		[groupsLookup]
	);

	const englishLevelOptions = useMemo(
		() => [
			...convertToSelectionModalOptions(englishLevels || [], 'name'),
			NO_VALUE_FILTER_OPTION,
		],
		[englishLevels]
	);

	const options = useMemo(
		(): LabboardGridOptions => ({
			users: userOptions,
			groups: groupOptions,
			englishLevels: englishLevelOptions,
			internshipTypes: LABBOARD_INTERNSHIP_TYPE_OPTIONS,
			finalResults: LABBOARD_FINAL_RESULT_OPTIONS,
		}),
		[userOptions, groupOptions, englishLevelOptions]
	);

	return { options };
};
