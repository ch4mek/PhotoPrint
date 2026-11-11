import { useTheme } from '@emotion/react';
import { AutoSelectWithChips } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useCallback, useMemo } from 'react';

import {
	DEFAULT_CONTENT_HEIGHT,
	discountFieldData,
	GENERAL_SECTION_TITLE,
	MAX_PERCENT_VALUE_LENGTH,
} from '../../constants';

import { Accordion } from '@/components/Accordion';
import { DynamicFieldsGroup } from '@/components/DynamicFieldsGroup';
import { ImageUpload } from '@/components/ImageUpload';
import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	AutoSelectGridWrapper,
	CollapsableDataWrapper,
	FirstRowGrid,
	FormContent,
	GridAreaWrapper,
	RemainingFormGrid,
	SecondRowGrid,
	TextFieldGridWrapper,
} from '@/forms/Discount/DiscountForm/styles';
import { getPopUpStylesConfig } from '@/forms/Discount/DiscountForm/utils/getStylesConfig';
import { EditorSection } from '@/forms/Discount/EditorSection';
import { checkIfOptionsEqualById } from '@/helpers/checkIfOptionEqualToValue';
import { extractKeyById } from '@/helpers/extractKey';
import { setNumbersType1Value } from '@/helpers/setNumbersType1Value';
import { useResize } from '@/hooks/useResize';
import { useGetDiscountCategories } from '@/queries/discounts/categories/useGetDiscountCategories';
import { useGetDiscountTagsByCategoryId } from '@/queries/discounts/tags/useGetDiscountTagsByCategoryId';
import { useGetDiscountCities } from '@/queries/discounts/useGetDiscountCities';
import { DiscountFormValues, DiscountSegmentTypes } from '@/types/discounts';

import { DiscountFormContentProps } from './types';

export const DiscountFormContent = ({
	formikProps,
	isLoading,
	isEditMode,
	isUploadImage,
	selectedImage,
	categoryId,
	discountData,
	parsedEditorData,
	setCategoryId,
	setEditorData,
	onImageChange,
	onImageRemove,
	onClose,
}: DiscountFormContentProps) => {
	const {
		submitForm,
		isSubmitting,
		setFieldValue,
		values,
		errors,
		touched,
		setFieldTouched,
		validateField,
		handleOutsideClose,
	} = formikProps;

	const [, windowHeight] = useResize();

	const theme = useTheme();

	const { data: discountCategories } = useGetDiscountCategories(
		undefined,
		DiscountSegmentTypes.Discount
	);

	const { data: discountCities } = useGetDiscountCities();

	const { data: categoriesTags } = useGetDiscountTagsByCategoryId(categoryId!, {
		enabled: categoryId != null,
	});

	const handleOnChange = (field: string) => (value: unknown) => {
		const isDiscountCategory =
			field === discountFieldData.discountCategory.name;

		if (isDiscountCategory) {
			if (value == null) {
				setFieldValue(discountFieldData.tags.name, []);
				setCategoryId(null);
			} else {
				const selectedCategory = value as { id: number };
				setFieldValue(discountFieldData.tags.name, []);
				setCategoryId(selectedCategory.id);
			}
		}

		setFieldValue(field, value);
		setFieldTouched(field, true);
		validateField(field);
	};

	const setPercentValue = useCallback(
		setNumbersType1Value(setFieldValue, MAX_PERCENT_VALUE_LENGTH),
		[setFieldValue]
	);

	const discountCitiesOptions = useMemo(
		() =>
			[...(discountCities || [])]?.map(({ name, cityId }) => ({
				name,
				id: cityId,
			})),
		[discountCities]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<PopUp
			type="form"
			showLoader={isSubmitting || isLoading || isUploadImage}
			title={
				isEditMode
					? getEditingPopUpTitle(DiscountSegmentTypes.Discount)
					: getAddingPopUpTitle(DiscountSegmentTypes.Discount)
			}
			onClose={onClose}
			onShadowClick={handleOutsideClose}
			controls={{
				positiveControl: { onClick: submitForm },
				negativeControl: { onClick: onClose },
			}}
			stylesConfig={popUpStylesConfig}
		>
			<FormContent>
				<Accordion
					label={GENERAL_SECTION_TITLE}
					isOpenByDefault
					defaultContentHeight={DEFAULT_CONTENT_HEIGHT}
				>
					<CollapsableDataWrapper>
						<ImageUpload
							file={selectedImage}
							removeFile={onImageRemove}
							handleFileChange={onImageChange}
						/>
						<FirstRowGrid>
							<AutoSelectGridWrapper
								{...discountFieldData.discountCategory}
								options={discountCategories || []}
								value={values.discountCategory}
								onChange={handleOnChange(
									discountFieldData.discountCategory.name
								)}
								isRequired
								error={
									Boolean(errors.discountCategory) && touched.discountCategory
								}
								errorText={errors.discountCategory}
								getKey={extractKeyById}
								checkIfOptionEqualToValue={checkIfOptionsEqualById}
							/>
							<TextFieldGridWrapper
								{...discountFieldData.name}
								onChange={setFieldValue}
								isRequired
							/>
						</FirstRowGrid>
						<SecondRowGrid>
							<TextFieldGridWrapper
								{...discountFieldData.discountPercent}
								onChange={setPercentValue}
								isRequired
							/>
							<TextFieldGridWrapper
								{...discountFieldData.discountPercentTo}
								onChange={setPercentValue}
							/>
							<TextFieldGridWrapper
								{...discountFieldData.companyType}
								onChange={setFieldValue}
							/>
						</SecondRowGrid>
						<RemainingFormGrid>
							<TextFieldGridWrapper
								{...discountFieldData.website}
								onChange={setFieldValue}
							/>
							<TextFieldGridWrapper
								{...discountFieldData.phoneNumber}
								onChange={setFieldValue}
							/>
							<TextFieldGridWrapper
								{...discountFieldData.location}
								onChange={setFieldValue}
							/>
							<DynamicFieldsGroup<DiscountFormValues>
								{...discountFieldData.mediaLinks}
							/>
							<GridAreaWrapper gridArea={discountFieldData.cities.gridArea}>
								<AutoSelectWithChips
									{...discountFieldData.cities}
									options={discountCitiesOptions}
									value={values.cities}
									onChange={handleOnChange(discountFieldData.cities.name)}
									isRequired
									error={Boolean(errors.cities && touched.cities)}
									errorText={String(errors.cities)}
									getKey={extractKeyById}
									checkIfOptionEqualToValue={checkIfOptionsEqualById}
								/>
							</GridAreaWrapper>
							<GridAreaWrapper gridArea={discountFieldData.tags.gridArea}>
								<AutoSelectWithChips
									{...discountFieldData.tags}
									value={values.tags}
									options={values.discountCategory ? categoriesTags ?? [] : []}
									onChange={handleOnChange(discountFieldData.tags.name)}
									getKey={extractKeyById}
									error={Boolean(errors.tags && touched.tags)}
									errorText={String(errors.tags)}
									checkIfOptionEqualToValue={checkIfOptionsEqualById}
								/>
							</GridAreaWrapper>
						</RemainingFormGrid>
					</CollapsableDataWrapper>
				</Accordion>
				{(discountData?.text || !isEditMode) && (
					<EditorSection
						isReadMode={false}
						setEditorData={setEditorData}
						initialData={parsedEditorData}
					/>
				)}
			</FormContent>
		</PopUp>
	);
};
