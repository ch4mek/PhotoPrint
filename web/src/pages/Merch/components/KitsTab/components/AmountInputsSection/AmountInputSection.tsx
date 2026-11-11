import { useFormikContext } from 'formik';

import { ONLY_DIGITS_WITH_EMPTY_VALUE_REG_EXP } from '@/constants/regexp';
import { TextField } from '@/fields/Text';
import { SIZE_INPUT_HEIGHT } from '@/forms/MerchElementForm/constants';
import { MerchKitsFormValues } from '@/forms/MerchKitsFrom/types';
import { MAX_NUMBER_TYPE_1_DIGITS } from '@/forms/schemas/constants';

import {
	AmountInputsWrapper,
	AmountInputWrapper,
	AmountPerStorageSectionWrapper,
	AmountSectionWrapper,
	StorageWrapper,
} from './styles';
import { AmountInputsSectionProps } from './types';

export function AmountInputsSection({
	merchStorages,
	disabled = false,
}: AmountInputsSectionProps) {
	const { setFieldValue, setFieldTouched, values } =
		useFormikContext<MerchKitsFormValues>();

	const handleSizeChange =
		(index: number) => (field: string, newValue: string) => {
			if (!ONLY_DIGITS_WITH_EMPTY_VALUE_REG_EXP.test(newValue)) {
				return;
			}

			setFieldValue(`minAmounts.${index}.minimumAmount`, newValue);
			setFieldTouched(field, true);
		};

	return (
		<AmountSectionWrapper>
			{merchStorages.map(({ id: configStorageId }, index) => (
				<AmountPerStorageSectionWrapper key={configStorageId}>
					<StorageWrapper variant="m-400">
						{merchStorages.find(({ id }) => id === configStorageId)?.name}
					</StorageWrapper>
					<AmountInputsWrapper>
						<AmountInputWrapper>
							<TextField
								height={SIZE_INPUT_HEIGHT}
								width="fullWidth"
								name={`minAmounts.${index}`}
								errorText={' '}
								value={`${values.minAmounts[index].minimumAmount}`}
								maxLength={MAX_NUMBER_TYPE_1_DIGITS}
								onChange={handleSizeChange(index)}
								inputMode="numeric"
								{...{ disabled }}
							/>
						</AmountInputWrapper>
					</AmountInputsWrapper>
				</AmountPerStorageSectionWrapper>
			))}
		</AmountSectionWrapper>
	);
}
