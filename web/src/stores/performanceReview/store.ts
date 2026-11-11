import { create } from 'zustand';

import { defaultMiddleware } from '../defaultMiddleware';

import { PerformanceReviewStore } from './types';

const STORE_TITLE = 'PerformanceReview';

export const usePerformanceReviewStore = create<PerformanceReviewStore>()(
	defaultMiddleware((set) => ({
		users: [],
		isShowIssuedStatus: false,
		setUsers: (users) =>
			set(
				(state) => {
					state.users = users;
				},
				undefined,
				`${STORE_TITLE}/setUsers`
			),
		toggleIsShowIssuedStatus: () =>
			set(
				(state) => {
					state.isShowIssuedStatus = !state.isShowIssuedStatus;
				},
				undefined,
				`${STORE_TITLE}/toggleIsShowIssuedStatus`
			),
	}))
);

usePerformanceReviewStore.devtools?.cleanup();
