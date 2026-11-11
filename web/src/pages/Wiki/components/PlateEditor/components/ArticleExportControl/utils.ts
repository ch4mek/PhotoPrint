import jsPDF from 'jspdf';

import { manropeFontBold } from '@/assets/fonts/editor/Manrope-Bold-normal';
import { manropeFontRegular } from '@/assets/fonts/editor/Manrope-Regular-normal';
import { manropeFontItalic } from '@/assets/fonts/editor/Monrope-Italic-normal';

import {
	FONT_TYPE_NAME,
	HEADLINES_SELECTOR,
	PDF_PAGE_WIDTH,
	SLATE_CODE_SELECTOR,
	SLATE_LEAF_SELECTOR,
	SLATE_STRING_ATTRIBUTE_SELECTOR,
	SLATE_STRING_SELECTOR,
	SLATE_TABLE_SELECTOR,
	STRONG_TAG_NAME,
	stylesDecorationsByTag,
	textDecorationElements,
	WIKI_COPY_PLATE_WRAPPER,
} from './constants';

const SLATE_CODE_BACKGROUND = '#ebebeb';
const WHITE_BACKGROUND = '#FFFFFF';

export const addCustomFonts = (doc: jsPDF) => {
	doc.addFileToVFS('Roboto-normal.ttf', manropeFontRegular);
	doc.addFont('Roboto-normal.ttf', FONT_TYPE_NAME, 'normal');

	doc.addFileToVFS('Roboto-bold.ttf', manropeFontBold);
	doc.addFont('Roboto-bold.ttf', FONT_TYPE_NAME, 'bold');

	doc.addFileToVFS('Roboto-bold.ttf', manropeFontItalic);
	doc.addFont('Roboto-bold.ttf', FONT_TYPE_NAME, 'italic');

	doc.setFont(FONT_TYPE_NAME, 'italic');
	doc.setFont(FONT_TYPE_NAME, 'normal');
	doc.setFont(FONT_TYPE_NAME, 'bold');
};

export const setCopyPlateWrapperToPdf = (clonedDoc: HTMLDocument) => {
	clonedDoc.querySelectorAll(`.${WIKI_COPY_PLATE_WRAPPER}`).forEach((el) => {
		const htmlEl = el as HTMLElement;
		htmlEl.style.width = `${PDF_PAGE_WIDTH}px`;
		htmlEl.style.display = `block`;
		htmlEl.style.padding = `0px 15px 0px 20px`;
		htmlEl.style.letterSpacing = '0.01px';
	});

	clonedDoc.querySelectorAll(`.${SLATE_TABLE_SELECTOR}`).forEach((el) => {
		const htmlEl = el as HTMLElement;
		htmlEl.style.border = `solid 0.5px black`;

		const cells = el.querySelectorAll('td, th');
		cells.forEach((cell) => {
			const htmlCell = cell as HTMLElement;
			htmlCell.style.border = 'solid 0.5px black';
		});
	});
};

export const removeEmptyStringsBackrounds = (clonedDoc: HTMLDocument) => {
	clonedDoc.querySelectorAll(SLATE_LEAF_SELECTOR).forEach((slateLeafSpan) => {
		const childSpans = slateLeafSpan.querySelectorAll('span');
		const slateLeafSpanHTML = slateLeafSpan as HTMLElement;

		childSpans.forEach((childSpan) => {
			const childSpanHTML = childSpan as HTMLElement;

			if (
				childSpanHTML.innerHTML.includes('&#xFEFF;') ||
				childSpanHTML.textContent?.includes('\uFEFF')
			) {
				slateLeafSpanHTML.style.backgroundColor = WHITE_BACKGROUND;
			}
		});
	});
};

export const setLinksToPdf = (clonedDoc: HTMLDocument) => {
	clonedDoc.querySelectorAll('a').forEach((aElem) => {
		const a = aElem as HTMLElement;
		const childWithSlateString = a.querySelector(SLATE_STRING_SELECTOR);

		if (childWithSlateString) {
			const originalText = childWithSlateString.textContent;
			const href = a.getAttribute('href') || '';

			a.innerHTML = '';
			a.textContent =
				originalText === href ? originalText : `${originalText} (${href})`;
			a.setAttribute('href', href);
		}
	});
};

export const setTextDecoration = (clonedDoc: HTMLDocument) => {
	clonedDoc.querySelectorAll(`.${SLATE_CODE_SELECTOR}`).forEach((el) => {
		const htmlEl = el as HTMLElement;
		htmlEl.style.fontFamily = `'Manrope', -apple-system, BlinkMacSystemFont, Roboto, 'Helvetica Neue', sans-serif`;
		htmlEl.style.backgroundColor = `${SLATE_CODE_BACKGROUND}`;
	});

	const applyStylesRecursively = (
		node: Element,
		inheritedStyles: Partial<CSSStyleDeclaration> = {}
	) => {
		const tag = node.tagName.toLowerCase();
		const currentStyles = { ...inheritedStyles };

		if (textDecorationElements.includes(tag)) {
			currentStyles.textDecoration = currentStyles.textDecoration
				? `${currentStyles.textDecoration} ${stylesDecorationsByTag[tag]}`
				: stylesDecorationsByTag[tag];
		}

		if (tag === STRONG_TAG_NAME) {
			currentStyles.fontWeight = stylesDecorationsByTag[tag];
		}

		if (node.hasAttribute(SLATE_STRING_ATTRIBUTE_SELECTOR)) {
			const htmlEl = node as HTMLElement;
			Object.assign(htmlEl.style, currentStyles);
		}

		Array.from(node.children).forEach((child) =>
			applyStylesRecursively(child, currentStyles)
		);
	};

	const root = clonedDoc.body;
	applyStylesRecursively(root);
};

export const setHeadlinesToPdf = (clonedDoc: HTMLDocument) => {
	clonedDoc.querySelectorAll(HEADLINES_SELECTOR).forEach((heading) => {
		const htmlHeading = heading as HTMLElement;

		const slateStringElement = htmlHeading.querySelector(SLATE_STRING_SELECTOR);

		if (slateStringElement) {
			const slateEl = slateStringElement as HTMLElement;
			slateEl.style.fontWeight = 'bold';
		}
	});
};
