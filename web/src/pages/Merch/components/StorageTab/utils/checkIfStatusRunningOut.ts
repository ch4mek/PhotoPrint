import { LOWERCASED_RUNNING_OUT_STATUS } from '../constants';

export const checkIfStatusRunningOut = (status?: string) =>
	status?.replace(' ', '').toLowerCase() === LOWERCASED_RUNNING_OUT_STATUS;
