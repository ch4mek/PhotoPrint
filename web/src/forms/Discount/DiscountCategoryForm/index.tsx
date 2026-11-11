import { AutoSelectWithChips } from '@mms/mms-ui-library/components/Autoselects';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import { ButtonAdd } from '@/components/ButtonAdd';
import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { getErrorMessageTheSameNameAlreadyExists } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { useToast } from '@/context/Toast';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import {
	DISCOUNT_CATEGORY_FIELD_TITLE,
	FORM_NAME,
	discountCategoryFieldData,
	fieldNames,
	initialFormValues,
} from '@/forms/Discount/DiscountCategoryForm/constants';
import {
	discountCategoryValidationSchema,
	tagNameSchema,
} from '@/forms/Discount/DiscountCategoryForm/schema';
import {
	TagFieldWrapper,
	TagsFieldWrapper,
} from '@/forms/Discount/DiscountCategoryForm/styles';
import {
	DiscountCategoryFormProps,
	HandleAddNewTagProps,
} from '@/forms/Discount/DiscountCategoryForm/types';
import { checkIsTagAlreadyExists } from '@/forms/Discount/DiscountCategoryForm/utils/checkIsTagAlreadyExists';
import { convertCategoriesToOptions } from '@/forms/Discount/DiscountCategoryForm/utils/convertCategoriesToOptions';
import { FormWrapper } from '@/forms/styles';
import { checkIfPrimitiveOptionsEqual } from '@/helpers/checkIfOptionEqualToValue';
import { createToast } from '@/helpers/createToast';
import { extractKeyFromPrimitive } from '@/helpers/extractKey';
import { useGetDiscountCategoryById } from '@/queries/discounts/categories/useGetDiscountCategoryById';
import { useGetDiscountTags } from '@/queries/discounts/tags/useGetDiscountTags';
import {
	DiscountCategoryFormValues,
	DiscountCategoryMutationData,
} from '@/types/discountCategories';

export function DiscountCategoryForm({
	handleClose,
	handleSubmit,
	categoryId,
}: DiscountCategoryFormProps) {
	const toast = useToast();
	const isEditMode = categoryId != null;

	const { data, isLoading, isFetching } = useGetDiscountCategoryById(
		categoryId!,
		{
			enabled: isEditMode,
		}
	);

	const {
		data: tagsData,
		isLoading: isTagsLoading,
		isFetching: isTagsFetching,
	} = useGetDiscountTags();

	const categoriesOptions = useMemo(
		() => convertCategoriesToOptions(tagsData),
		[tagsData]
	);

	const initialFormData: DiscountCategoryFormValues = useMemo(() => {
		if (isEditMode && data) {
			return {
				name: data.name,
				discountCategoryTags: data.discountCategoryTags.map(({ name }) => name),
				tagName: '',
			};
		}

		return initialFormValues;
	}, [isEditMode, data]);

	const handleFormSubmit = useCallback(
		async (
			values: DiscountCategoryFormValues,
			{ setSubmitting }: FormikHelpers<DiscountCategoryFormValues>
		) => {
			try {
				setSubmitting(true);

				const { discountCategoryTags, name } = values;

				await handleSubmit({
					orderWeight: 0,
					...(data || {}),
					name,
					discountCategoryTags,
				} as DiscountCategoryMutationData);
			} finally {
				setSubmitting(false);
			}
		},
		[handleSubmit]
	);

	const handleAddNewTag = useCallback(
		({
				values,
				setFieldValue,
				setFieldError,
				setFieldTouched,
			}: HandleAddNewTagProps) =>
			() => {
				const newTag = values.tagName.trim();
				const isTagAlreadyExists = checkIsTagAlreadyExists(
					[...categoriesOptions, ...values.discountCategoryTags],
					newTag
				);

				if (isTagAlreadyExists) {
					setFieldError(fieldNames.tagName, ' ');
					toast.open(
						createToast(
							NotificationType.ERROR,
							getErrorMessageTheSameNameAlreadyExists('Tag')
						)
					);
					setFieldTouched(fieldNames.tagName, true);

					return;
				}

				setFieldValue(fieldNames.tags, [
					...values.discountCategoryTags,
					newTag.toLowerCase(),
				]);

				setFieldValue(fieldNames.tagName, '');
				setFieldTouched(fieldNames.tagName, false);
			},
		[categoriesOptions]
	);

	const isShowLoader =
		isLoading || isFetching || isTagsLoading || isTagsFetching;

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormData}
			validationSchema={discountCategoryValidationSchema}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={categoryId}
		>
			{({
				submitForm,
				isSubmitting,
				setFieldValue,
				values,
				errors,
				validateField,
				setFieldTouched,
				touched,
				setFieldError,
				handleOutsideClose,
			}) => {
				const handleCategoryTagsChange = (value: string[]) => {
					setFieldValue(fieldNames.tags, value);
					setFieldTouched(fieldNames.tags, true);
					validateField(fieldNames.tags);
				};

				const handleNewTagNameChange = async (
					field: string,
					newValue: string
				) => {
					setFieldValue(field, newValue, false);

					if (!newValue) {
						setFieldError(field, '');
						setFieldTouched(field, false, false);

						return;
					}

					try {
						await tagNameSchema.validate(newValue);
						setFieldError(field, '');
					} catch (error: any) {
						setFieldError(field, error.message);
					}

					setFieldTouched(field, true, false);
				};

				return (
					<PopUp
						type="form"
						showLoader={isSubmitting || isShowLoader}
						title={
							isEditMode
								? getEditingPopUpTitle(DISCOUNT_CATEGORY_FIELD_TITLE)
								: getAddingPopUpTitle(DISCOUNT_CATEGORY_FIELD_TITLE)
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
					>
						<FormWrapper>
							<TextField
								{...discountCategoryFieldData.categoryName}
								onChange={setFieldValue}
								isRequired
							/>
							<TagsFieldWrapper>
								<TagFieldWrapper>
									<TextField
										{...discountCategoryFieldData.newTagName}
										onChange={handleNewTagNameChange}
										error={Boolean(errors.tagName && touched.tagName)}
										errorText={errors.tagName}
									/>
								</TagFieldWrapper>
								<ButtonAdd
									view="outline"
									disabled={
										!values.tagName.trim() ||
										!!(errors.tagName && touched.tagName)
									}
									onAdd={handleAddNewTag({
										values,
										setFieldValue,
										setFieldError,
										setFieldTouched,
									})}
								/>
							</TagsFieldWrapper>
							<AutoSelectWithChips
								{...discountCategoryFieldData.tags}
								value={values.discountCategoryTags}
								options={categoriesOptions || []}
								onChange={handleCategoryTagsChange}
								getKey={extractKeyFromPrimitive}
								error={Boolean(
									errors.discountCategoryTags && touched.discountCategoryTags
								)}
								errorText={String(errors.discountCategoryTags)}
								checkIfOptionEqualToValue={checkIfPrimitiveOptionsEqual}
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
