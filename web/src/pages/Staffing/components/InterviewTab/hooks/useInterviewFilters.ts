import { useEffect, useMemo } from 'react';

import { createSelector } from '@/stores/helpers';
import { useInterviewStore } from '@/stores/interview/store';

export const useInterviewFilters = () => {
	const { initialize, initialFilters } = useInterviewStore(
		createSelector('initialize', 'initialFilters')
	);

	const filters = useInterviewStore(
		createSelector(
			'id',
			'user',
			'specialization',
			'client',
			'jiraTask',
			'type',
			'status',
			'date',
			'time',
			'saleManager',
			'reasonForRefusal',
			'note'
		)
	);

	const storeSetters = useInterviewStore(
		createSelector(
			'setId',
			'setUser',
			'setSpecialization',
			'setClient',
			'setJiraTask',
			'setType',
			'setStatus',
			'setDate',
			'setTime',
			'setSaleManager',
			'setReasonForRefusal',
			'setNote',
			'setAll',
			'reset'
		)
	);

	const setFilters = useMemo(
		() => ({
			id: storeSetters.setId,
			user: storeSetters.setUser,
			specialization: storeSetters.setSpecialization,
			client: storeSetters.setClient,
			jiraTask: storeSetters.setJiraTask,
			type: storeSetters.setType,
			status: storeSetters.setStatus,
			date: storeSetters.setDate,
			time: storeSetters.setTime,
			saleManager: storeSetters.setSaleManager,
			reasonForRefusal: storeSetters.setReasonForRefusal,
			note: storeSetters.setNote,
			set: storeSetters.setAll,
			reset: storeSetters.reset,
		}),
		[storeSetters]
	);

	useEffect(() => {
		initialize(initialFilters);
	}, [initialFilters, initialize]);

	return { filters, setFilters };
};
