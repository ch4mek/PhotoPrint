import {
	createContext,
	PropsWithChildren,
	useContext,
	useMemo,
	useState,
} from 'react';

import type { EquipmentContextValues } from '../types';

import { getValidationSchema } from '@/forms/EquipmentForm/MainForm/schema';
import { MainEquipmentFormValues } from '@/forms/EquipmentForm/MainForm/types';
import { checkNullableField } from '@/forms/EquipmentForm/utils/checkNullableField';

import {
	DEFAULT_CURRENCY_BYN,
	DEFAULT_CURRENCY_USD,
	PURCHASE_CURRENCY_KEY,
	PURCHASE_PRICE_USD_KEY,
} from './constants';

const EquipmentContext = createContext({} as EquipmentContextValues);

function EquipmentProvider({ children }: PropsWithChildren) {
	const [equipmentModel, setEquipmentModel] = useState<MainEquipmentFormValues>(
		Object.keys(getValidationSchema().fields).reduce(
			(acc: any, value: string) => {
				acc[value] = checkNullableField(value) ? null : '';

				if (value === PURCHASE_PRICE_USD_KEY) {
					acc[value] = DEFAULT_CURRENCY_USD;
				}

				if (value === PURCHASE_CURRENCY_KEY) {
					acc[value] = DEFAULT_CURRENCY_BYN;
				}

				return acc;
			},
			{}
		)
	);

	const value = useMemo(
		() => ({ equipmentModel, setEquipmentModel }),
		[equipmentModel, setEquipmentModel]
	);

	return (
		<EquipmentContext.Provider value={value}>
			{children}
		</EquipmentContext.Provider>
	);
}

const useEquipmentContext = () => useContext(EquipmentContext);

export { EquipmentProvider, useEquipmentContext };
