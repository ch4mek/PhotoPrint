import { useFormikContext } from 'formik';
import { useMemo } from 'react';

import { filterMerchValuesDueToIsClothing } from '../../utils/filterMerchValuesDueToIsClothing';
import { findValueByIndex } from '../../utils/findValueByIndex';

import { ONLY_DIGITS_WITH_EMPTY_VALUE_REG_EXP } from '@/constants/regexp';
import { TextField } from '@/fields/Text';
import { SIZE_INPUT_HEIGHT } from '@/forms/MerchElementForm/constants';
import { MerchElementsFormValues } from '@/forms/MerchElementForm/types';
import { MAX_NUMBER_TYPE_1_DIGITS } from '@/forms/schemas/constants';

import {
	SizeIdWrapper,
	SizeInputsWrapper,
	SizeInputWrapper,
	SizePerStorageSectionWrapper,
	SizeSectionWrapper,
	StorageWrapper,
} from './styles';
import { SizesInputsSectionProps } from './types';

export function SizesInputsSection({
	merchStorages,
	disabled,
}: SizesInputsSectionProps) {
	const { values, setFieldValue, setFieldTouched } =
		useFormikContext<MerchElementsFormValues>();

	const { isClothing } = values;

	const handleSizeChange =
		(index: number) => (field: string, newValue: string) => {
			if (!ONLY_DIGITS_WITH_EMPTY_VALUE_REG_EXP.test(newValue)) {
				return;
			}

			setFieldValue(`sizeMinAmounts.${index}.minAmount`, newValue);
			setFieldTouched(field, true);
		};

	const filteredMerchSizes = useMemo(
		() => filterMerchValuesDueToIsClothing(values),
		[values, isClothing]
	);

	return (
		<SizeSectionWrapper isClothing={isClothing}>
			{merchStorages.map(({ id: configStorageId }) => (
				<SizePerStorageSectionWrapper
					isClothing={isClothing}
					key={configStorageId}
				>
					<StorageWrapper variant="m-400" isClothing={isClothing}>
						{merchStorages.find(({ id }) => id === configStorageId)?.name}
					</StorageWrapper>
					<SizeInputsWrapper>
						{filteredMerchSizes.map(
							({ sizeId, storageId, minAmount }) =>
								Number(configStorageId) === storageId && (
									<SizeInputWrapper
										isClothing={isClothing}
										key={`${sizeId}.${storageId}`}
									>
										<SizeIdWrapper variant="m-400">
											{isClothing && sizeId}
										</SizeIdWrapper>
										<TextField
											height={SIZE_INPUT_HEIGHT}
											width="fullWidth"
											name={`sizeMinAmounts.${findValueByIndex(
												values,
												sizeId,
												storageId
											)}`}
											errorText={' '}
											maxLength={MAX_NUMBER_TYPE_1_DIGITS}
											value={String(minAmount)}
											onChange={handleSizeChange(
												findValueByIndex(values, sizeId, storageId)
											)}
											inputMode="numeric"
											{...{ disabled }}
										/>
									</SizeInputWrapper>
								)
						)}
					</SizeInputsWrapper>
				</SizePerStorageSectionWrapper>
			))}
		</SizeSectionWrapper>
	);
}
