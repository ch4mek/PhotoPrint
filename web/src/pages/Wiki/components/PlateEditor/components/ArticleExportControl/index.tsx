import { useTheme } from '@emotion/react';
import { Plate } from '@udecode/plate';
import { jsPDF as JsPdfGenerator } from 'jspdf';
import { memo, useRef } from 'react';

import { ExportControl } from '@/components/ExportControl';
import {
	CLONE_EDITOR_ID,
	initialEditorData,
	lightTextColorPalette,
} from '@/constants/plateEditor';
import { getCustomPluginsWithThemeMode } from '@/plugins/PlateEditorPlugins';
import { CONFIG } from '@/plugins/PlateEditorPlugins/config';
import { CustomValue } from '@/types/plateEditor';

import {
	exportOptions,
	JsPdfGeneratorConfig,
	WIKI_COPY_PLATE_WRAPPER,
	wikiHtmlDocConfig,
} from './constants';
import { PlateWrapper } from './styles';
import {
	addCustomFonts,
	removeEmptyStringsBackrounds,
	setCopyPlateWrapperToPdf,
	setHeadlinesToPdf,
	setLinksToPdf,
	setTextDecoration,
} from './utils';

function ArticleExportControlInner({
	title,
	editorData,
}: {
	title: string;
	editorData: CustomValue;
}) {
	const theme = useTheme();

	const wikiArticleRef = useRef(null);
	const defaultColor = lightTextColorPalette[1];

	const exportToPDF = async () => {
		if (!wikiArticleRef.current) {
			return;
		}

		const doc = new JsPdfGenerator(JsPdfGeneratorConfig);

		addCustomFonts(doc);

		doc.text(title, 10, 30);

		await doc.html(wikiArticleRef.current, {
			margin: wikiHtmlDocConfig.margin,
			autoPaging: wikiHtmlDocConfig.autoPaging,
			html2canvas: {
				letterRendering: true,
				scale: wikiHtmlDocConfig.scale,
				useCORS: true,
				allowTaint: true,
				onclone: (clonedDoc) => {
					setCopyPlateWrapperToPdf(clonedDoc);
					removeEmptyStringsBackrounds(clonedDoc);
					setTextDecoration(clonedDoc);
					setLinksToPdf(clonedDoc);
					setHeadlinesToPdf(clonedDoc);
				},
			},

			callback: (finalDoc) => {
				finalDoc.save(`${title}.pdf`);
			},
		});
	};

	return (
		<>
			<ExportControl options={exportOptions} onChange={exportToPDF} />
			<PlateWrapper ref={wikiArticleRef} className={WIKI_COPY_PLATE_WRAPPER}>
				<Plate<CustomValue>
					id={CLONE_EDITOR_ID}
					editableProps={{
						...CONFIG.editableProps,
						readOnly: true,
						style: {
							...CONFIG.editableProps.style,
							color: defaultColor,
						},
					}}
					value={editorData || initialEditorData}
					plugins={getCustomPluginsWithThemeMode(theme.mode, true)}
					normalizeInitialValue={false}
				/>
			</PlateWrapper>
		</>
	);
}

export const ArticleExportControl = memo(ArticleExportControlInner);
