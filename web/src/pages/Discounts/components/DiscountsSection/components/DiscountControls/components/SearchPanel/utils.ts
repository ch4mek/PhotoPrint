import JsPDF from 'jspdf';

import { manropeFontRegular } from '@/assets/fonts/editor/Manrope-Regular-normal';
import { Discount } from '@/types/discounts';
import { CustomRootBlock, DiscountNode } from '@/types/plateEditor';

import {
	discountsPDFConfig,
	fontConfig,
	lineTitles,
	PDF_FILE_NAME,
	PDF_FONT_SIZE,
	pdfLayoutConfig,
} from './constants';

const extractTextFromNode = (node: DiscountNode): string => {
	if (!node) {
		return '';
	}

	if (typeof node.text === 'string') {
		return node.text;
	}

	if (Array.isArray(node.children)) {
		return node.children.map(extractTextFromNode).join('');
	}

	return '';
};

const parseDescription = (textJson?: string): string[] => {
	if (!textJson) {
		return [];
	}

	try {
		const parsed: CustomRootBlock[] = JSON.parse(textJson);

		if (!Array.isArray(parsed)) {
			return [];
		}

		return parsed
			.map((block) => extractTextFromNode(block as DiscountNode))
			.filter((text) => text.trim() !== '');
	} catch (error) {
		return [];
	}
};

export const generateDiscountsPDF = (discounts: Discount[]) => {
	if (!discounts || discounts.length === 0) {
		return;
	}

	const doc = new JsPDF(discountsPDFConfig);

	const { filename, fontName, fontType } = fontConfig;

	doc.addFileToVFS(filename, manropeFontRegular);
	doc.addFont(filename, fontName, fontType);
	doc.setFont(fontName, fontType);
	doc.setFontSize(PDF_FONT_SIZE);

	const normalizeY = (y: number) => {
		if (y > pdfLayoutConfig.pageHeightLimit) {
			doc.addPage();

			return pdfLayoutConfig.startY;
		}

		return y;
	};

	discounts.reduce((currentY: number, discount: Discount) => {
		const {
			name,
			discountPercentTo,
			discountPercent,
			discountCities,
			companyType,
			discountCategoryName,
			location,
			website,
			phoneNumber,
			text,
		} = discount;

		const cities = Array.from(
			new Set(discountCities.map((city) => city.name))
		).join(', ');

		const percent = discountPercentTo
			? `${discountPercent}-${discountPercentTo}%`
			: `${discountPercent}%`;

		const lines = [
			`${lineTitles.name}: ${name}`,
			`${lineTitles.percent}: ${percent}`,
			cities ? `${lineTitles.cities}: ${cities}` : null,
			companyType ? `${lineTitles.companyType}: ${companyType}` : null,
			discountCategoryName
				? `${lineTitles.category}: ${discountCategoryName}`
				: null,
			location ? `${lineTitles.location}: ${location}` : null,
			website ? `${lineTitles.website}: ${website}` : null,
			phoneNumber ? `${lineTitles.phoneNumber}: ${phoneNumber}` : null,
		];

		const descriptionParagraphs = parseDescription(text);
		const textBlock = [
			...lines,
			descriptionParagraphs.length > 0 ? `\n${lineTitles.description}:` : null,
			...descriptionParagraphs,
		].filter((line): line is string => line !== null);

		const yAfterText = textBlock.reduce((lineY, line) => {
			const splitLines: string[] = doc.splitTextToSize(
				line,
				pdfLayoutConfig.maxTextWidth
			);

			return splitLines.reduce((splitY, splitLine) => {
				const yForLine = normalizeY(splitY);
				doc.text(splitLine, pdfLayoutConfig.startX, yForLine);

				return yForLine + pdfLayoutConfig.lineHeight;
			}, lineY);
		}, currentY);

		const yForDivider = normalizeY(yAfterText);
		doc.line(
			pdfLayoutConfig.startX,
			yForDivider,
			pdfLayoutConfig.lineEndX,
			yForDivider
		);

		return yForDivider + pdfLayoutConfig.gapAfterDiscount;
	}, pdfLayoutConfig.startY);

	doc.save(`${PDF_FILE_NAME}.pdf`);
};
