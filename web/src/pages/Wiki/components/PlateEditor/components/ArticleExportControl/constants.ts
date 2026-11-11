import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import { jsPDFOptions } from 'jspdf';

export const exportOptions: PickerButtonOption[] = [{ value: 'pdf' }];
export const PDF_PAGE_WIDTH = 830;

export const WIKI_COPY_PLATE_WRAPPER = 'wiki-copy-plate-wrapper';
export const SLATE_STRING_SELECTOR = '[data-slate-string="true"]';
export const SLATE_LEAF_SELECTOR = 'span[data-slate-leaf="true"]';

export const SLATE_STRING_ATTRIBUTE_SELECTOR = 'data-slate-string';
export const SLATE_CODE_SELECTOR = 'slate-code';
export const SLATE_TABLE_SELECTOR = 'slate-table';
export const STRONG_TAG_NAME = 'strong';
export const HEADLINES_SELECTOR = 'h1, h2, h3, h4, h5, h6';

export const FONT_TYPE_NAME = 'Roboto';

export const textDecorationElements = ['u', 's', 'strike'];

export const wikiHtmlDocConfig: {
	margin: number[];
	scale: number;
	autoPaging: 'text' | 'slice';
} = {
	margin: [40, 0, 20, 0],
	scale: 0.53,
	autoPaging: 'text',
};

export const JsPdfGeneratorConfig: jsPDFOptions = {
	format: 'a4',
	unit: 'px',
};

export const stylesDecorationsByTag: Record<string, string> = {
	u: 'underline',
	s: 'line-through',
	strike: 'line-through',
	strong: 'bold',
};
