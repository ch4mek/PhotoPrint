import { create } from 'zustand';

import { defaultMiddleware } from '../defaultMiddleware';

import { DEFAULT_INTERVIEW_GRID_FILTERS } from '@/pages/Staffing/components/InterviewTab/constants';

import { InterviewFilterStore } from './types';

const INTERVIEW_STORE_TITLE = 'interviewStore';

export const useInterviewStore = create<InterviewFilterStore>()(
	defaultMiddleware((set, get) => ({
		...DEFAULT_INTERVIEW_GRID_FILTERS,
		initialFilters: DEFAULT_INTERVIEW_GRID_FILTERS,

		setId: (id) => set({ id }, false, `${INTERVIEW_STORE_TITLE}/setId`),
		setUser: (user) => set({ user }, false, `${INTERVIEW_STORE_TITLE}/setUser`),
		setSpecialization: (specialization) =>
			set(
				{ specialization },
				false,
				`${INTERVIEW_STORE_TITLE}/setSpecialization`
			),
		setClient: (client) =>
			set({ client }, false, `${INTERVIEW_STORE_TITLE}/setClient`),
		setJiraTask: (jiraTask) =>
			set({ jiraTask }, false, `${INTERVIEW_STORE_TITLE}/setJiraTask`),
		setType: (type) => set({ type }, false, `${INTERVIEW_STORE_TITLE}/setType`),
		setStatus: (status) =>
			set({ status }, false, `${INTERVIEW_STORE_TITLE}/setStatus`),
		setDate: (date) => set({ date }, false, `${INTERVIEW_STORE_TITLE}/setDate`),
		setTime: (time) => set({ time }, false, `${INTERVIEW_STORE_TITLE}/setTime`),
		setSaleManager: (saleManager) =>
			set({ saleManager }, false, `${INTERVIEW_STORE_TITLE}/setSaleManager`),
		setReasonForRefusal: (reasonForRefusal) =>
			set(
				{ reasonForRefusal },
				false,
				`${INTERVIEW_STORE_TITLE}/setReasonForRefusal`
			),
		setNote: (note) => set({ note }, false, `${INTERVIEW_STORE_TITLE}/setNote`),

		setAll: (filters) =>
			set({ ...filters }, false, `${INTERVIEW_STORE_TITLE}/setAll`),

		reset: () => {
			const { initialFilters } = get();
			set({ ...initialFilters }, false, `${INTERVIEW_STORE_TITLE}/reset`);
		},

		initialize: (initialFilters) =>
			set(
				{
					...initialFilters,
					initialFilters,
				},
				false,
				`${INTERVIEW_STORE_TITLE}/initialize`
			),
	}))
);
