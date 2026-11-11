import uniq from 'lodash/uniq';

import { MinAmountItemResponse } from '../types';

export const getUniqueStorages = (
	odpItemSizeMinAmounts: MinAmountItemResponse[]
) => uniq(odpItemSizeMinAmounts.map(({ storageName }) => storageName));
