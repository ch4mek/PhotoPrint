import { Theme } from '@emotion/react';

import { PossibleKitsItemResponse } from '../../KitsTab/types';
import { MinAmountItemResponse } from '../types';

import { checkIfStatusRunningOut } from './checkIfStatusRunningOut';
import { getCellColorByAmount } from './getCellColorByAmount';

export const getCellColorForRow = (
	responseItems: Partial<
		PossibleKitsItemResponse &
			Omit<MinAmountItemResponse, keyof PossibleKitsItemResponse>
	>[],
	theme: Theme,
	expanded?: boolean,
	isDisabled?: boolean
): ReturnType<typeof getCellColorByAmount> => {
	if (expanded || isDisabled) {
		return undefined;
	}

	const runningOutItems = responseItems.filter(({ status }) =>
		checkIfStatusRunningOut(status)
	);

	if (!runningOutItems.length) {
		return undefined;
	}

	const runOutItem = runningOutItems.find(
		({ amount, possibleKitAmount }) => amount === 0 || possibleKitAmount === 0
	);
	const colorItem = runOutItem ?? runningOutItems[0];

	return getCellColorByAmount(
		theme,
		colorItem?.amount ?? colorItem.possibleKitAmount,
		colorItem?.status
	);
};
