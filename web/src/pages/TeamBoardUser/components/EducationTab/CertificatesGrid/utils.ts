import isFunction from 'lodash/isFunction';
import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';
import { CertificateFilters } from '@/types/certificate';

import { defaultUserCertificatesFilters } from './constants';
import {
	UserCertificatesFilterActions,
	UserCertificatesFiltersHandlers,
	UserCertificatesReducerAction,
} from './types';

export function userCertificatesReducer(
	state: CertificateFilters,
	{ type, payload }: UserCertificatesReducerAction
): CertificateFilters {
	switch (type) {
		case UserCertificatesFilterActions.RESET:
			return { ...payload };
		case UserCertificatesFilterActions.CERTIFICATE_ID:
			return {
				...state,
				certificateId: payload,
			};
		case UserCertificatesFilterActions.STACK:
			return {
				...state,
				stack: payload,
			};
		case UserCertificatesFilterActions.RECEIPT_DATE:
			return {
				...state,
				receiptDate: {
					...state.receiptDate,
					...payload,
				},
			};
		default:
			return state;
	}
}

const handleSetStateAction = <T>(
	prevValue: T,
	payload: SetStateAction<T>
): T => {
	if (isFunction(payload)) {
		return payload(prevValue);
	}

	return payload;
};

export const createUserCertificatesFiltersHandlers = (
	state: CertificateFilters,
	dispatch: Dispatch<UserCertificatesReducerAction>
): UserCertificatesFiltersHandlers => ({
	reset: () =>
		dispatch({
			type: UserCertificatesFilterActions.RESET,
			payload: defaultUserCertificatesFilters,
		}),
	certificateId: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.certificateId, payloadForAction);

		dispatch({ type: UserCertificatesFilterActions.CERTIFICATE_ID, payload });
	},
	stack: (payloadForAction: SetStateAction<string>) => {
		const payload = handleSetStateAction(state.stack, payloadForAction);

		dispatch({ type: UserCertificatesFilterActions.STACK, payload });
	},
	receiptDate: (payloadForAction: SetStateAction<FromToProps<Date>>) => {
		const payload = handleSetStateAction(state.receiptDate, payloadForAction);

		dispatch({ type: UserCertificatesFilterActions.RECEIPT_DATE, payload });
	},
});

export const isResetDisabled = (filters: CertificateFilters) => {
	const keys = Object.keys(filters) as Array<keyof typeof filters>;

	return keys.every(
		(key) => filters[key] === defaultUserCertificatesFilters[key]
	);
};
