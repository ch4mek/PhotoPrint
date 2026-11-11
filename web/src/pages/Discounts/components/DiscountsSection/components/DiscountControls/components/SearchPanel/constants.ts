import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { jsPDFOptions } from 'jspdf';

import { DiscountFilterParam } from '@/queries/discounts/types';

export const exportOptions: PickerButtonOption[] = [{ value: 'pdf' }];

export const discountsPDFConfig: jsPDFOptions = {
	format: 'a4',
	unit: 'pt',
};

export const emptyDiscountsFilters: DiscountFilterParam = {
	cityId: null,
	tagId: null,
	discountCategoryId: null,
	name: '',
};

export const PDF_FONT_SIZE = 14;
export const PDF_FILE_NAME = 'Modsen Discounts';

export const pdfLayoutConfig = {
	startX: 40,
	startY: 40,
	lineHeight: 16,
	maxTextWidth: 500,
	lineEndX: 550,
	pageHeightLimit: 760,
	gapAfterDiscount: 40,
} as const;

export const lineTitles = {
	name: 'Название',
	percent: 'Процент скидки',
	cities: 'Города действия скидки',
	companyType: 'Тип компании',
	category: 'Категория скидки',
	location: 'Локация',
	website: 'Официальный сайт',
	phoneNumber: 'Телефон',
	description: 'Описание',
};

export const fontConfig = {
	filename: 'Roboto-normal.ttf',
	fontName: 'Roboto',
	fontType: 'normal',
};
