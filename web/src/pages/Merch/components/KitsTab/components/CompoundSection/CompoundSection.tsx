import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Counter } from '@mms/mms-ui-library/components/Counter';
import { BasketIcon } from '@mms/mms-ui-library/icons/base/BasketIcon';
import { useFormikContext, FieldArray } from 'formik';
import { useMemo } from 'react';

import { AddMoreButton } from '@/components/AddMoreButton';
import { FIELD_IS_REQUIRED } from '@/constants/errors';
import { merchKitsFieldData } from '@/forms/MerchKitsFrom/constants';
import { MerchKitsFormValues } from '@/forms/MerchKitsFrom/types';
import { MerchElementsLookupResponse } from '@/pages/Merch/components/StorageTab/types';
import { useMerchElementsLookup } from '@/queries/merchElements/useGetMerchElementsLookup';

import {
	MAX_COMPOUND_AMOUNT_IN_KIT,
	MAX_COMPOUNDS,
	MIN_COMPOUND_AMOUNT_IN_KIT,
} from './constants';
import {
	CompoundItemWrapper,
	CompoundSectionWrapper,
	CompoundsFieldsWrapper,
	CompoundWrapper,
	Divider,
} from './styles';
import { CompoundsSectionProps } from './types';
import { getCompoundAutoSelectValueName } from './utils/getCompoundAutoSelectValueName';

export function CompoundsSection({
	disabled = false,
	kitCompounds,
}: CompoundsSectionProps) {
	const {
		values: { compounds },
		errors,
		setFieldValue,
		setFieldTouched,
		touched,
	} = useFormikContext<MerchKitsFormValues>();

	const { data: allMerchElements } = useMerchElementsLookup();

	const filteredAllMerchElements = useMemo(
		() =>
			allMerchElements?.filter(
				({ id }) => !compounds.some(({ itemId }) => id === itemId)
			),
		[allMerchElements, compounds]
	);

	return (
		<CompoundSectionWrapper>
			<FieldArray name="compounds">
				{(arrayHelpers) => {
					const handleAddFieldButtonClick = () => {
						arrayHelpers.push({
							amountInKit: 1,
							itemId: null,
						});
					};

					const handleDeleteStageButtonClick = (deletedId: number) => () => {
						arrayHelpers.remove(deletedId);
					};

					const handleChangeCompoundName =
						(index: number) => (value: unknown) => {
							const newValue = value as MerchElementsLookupResponse;
							setFieldValue(
								`compounds.${index}.itemId`,
								value ? newValue.id : ''
							);
							setFieldTouched(`compounds.${index}`, true);
						};

					const handleChangeCompoundAmount =
						(index: number) => (newValue: unknown) => {
							setFieldValue(`compounds.${index}.amountInKit`, newValue);
						};

					return (
						<>
							<CompoundsFieldsWrapper>
								{compounds?.map((compound, index) => (
									<CompoundWrapper key={compound.itemId}>
										<CompoundItemWrapper>
											<Autoselect
												{...merchKitsFieldData.compound}
												name={`compounds.${index}`}
												options={filteredAllMerchElements ?? []}
												valueField="name"
												onChange={handleChangeCompoundName(index)}
												value={`${
													getCompoundAutoSelectValueName(
														compounds,
														index,
														allMerchElements,
														kitCompounds
													) ?? ''
												}`}
												error={Boolean(
													errors.compounds?.[index] &&
														touched.compounds?.[index]
												)}
												errorText={FIELD_IS_REQUIRED}
												{...{ disabled }}
												isRequired
											/>
											<Counter
												value={compounds[index].amountInKit}
												onChange={handleChangeCompoundAmount(index)}
												maxValue={MAX_COMPOUND_AMOUNT_IN_KIT}
												minValue={MIN_COMPOUND_AMOUNT_IN_KIT}
												editable
												{...{ disabled }}
											/>
											{compounds?.length > 1 && (
												<ActionButton
													onClick={handleDeleteStageButtonClick(index)}
													{...{ disabled }}
												>
													<BasketIcon />
												</ActionButton>
											)}
										</CompoundItemWrapper>
										{index !== compounds.length - 1 && <Divider />}
									</CompoundWrapper>
								))}
							</CompoundsFieldsWrapper>
							{!disabled && compounds && compounds?.length < MAX_COMPOUNDS && (
								<AddMoreButton handleClick={handleAddFieldButtonClick} />
							)}
						</>
					);
				}}
			</FieldArray>
		</CompoundSectionWrapper>
	);
}
