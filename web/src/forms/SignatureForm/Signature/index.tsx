import { lightPalette } from '@mms/mms-ui-library/theme';
import { useCallback, useMemo } from 'react';

import line from '@/assets/signature/line.svg';
import logo from '@/assets/signature/logo.svg';
import linePNG from '@/assets/signature/png/line.png';
import logoPNG from '@/assets/signature/png/logo.png';
import { MODSEN_URL } from '@/forms/SignatureForm/constants';

import { HTML_FILE_STYLES, logos, signatureSizes } from './constants';
import {
	TableSignature,
	TableRowSignature,
	TableSignatureInner,
} from './styles';
import type { SignatureLinkProps, SignatureProps } from './types';

export function Signature({
	fullName,
	position,
	website,
	phone,
	mode = 'default',
}: SignatureProps) {
	const getLinkComponent = useCallback(
		({ value, type = 'website' }: SignatureLinkProps) => {
			const href = type === 'phone' ? `tel:${value}` : value;
			const target = type === 'website' ? '_blank' : '_self';
			const renderValue = type === 'website' ? new URL(value).hostname : value;
			const linkClassName = `signature-link-${type}`;

			return (
				<div className="signature-link-wrapper">
					<a href={href} target={target} className={linkClassName}>
						{renderValue}
					</a>
				</div>
			);
		},
		[]
	);

	const phoneValue = useMemo(
		() => getLinkComponent({ value: phone, type: 'phone' }),
		[phone]
	);

	const websiteValue = useMemo(
		() => getLinkComponent({ value: website, type: 'website' }),
		[website]
	);

	return (
		<TableSignature
			className="sigrature-table"
			cellPadding="0"
			cellSpacing="0"
			id="signature"
		>
			<style>{HTML_FILE_STYLES}</style>
			<tbody>
				<TableRowSignature style={{ textAlign: 'end' }}>
					<td
						style={{
							display: 'flex',
							alignItems: 'center',
							height: 10,
							marginBottom: 8,
							width: signatureSizes.l,
						}}
					>
						<img
							src={mode === 'email' ? linePNG : line}
							alt="signature-line"
							style={{
								width: signatureSizes.l,
							}}
						/>
					</td>
				</TableRowSignature>
				<TableRowSignature>
					<td>
						<TableSignatureInner>
							<tbody>
								<TableRowSignature className="signature-row">
									<td className="signature-fullname-wrapper">
										<b className="signature-fullname">{fullName}</b>
									</td>
									<td className="signature-logo-wrapper">
										<img
											src={mode === 'email' ? logoPNG : logo}
											width="92"
											height="20"
											alt="Modsen logo"
										/>
									</td>
								</TableRowSignature>
								<TableRowSignature className="signature-row">
									<td>
										{mode === 'default' && (
											<span className="signature-position">{position}</span>
										)}

										{mode === 'email' && (
											// Outlook allows to change only links color
											<a
												href={MODSEN_URL}
												className="signature-position"
												style={{
													color: `${lightPalette['graphics-accent']} !important`,
													textDecoration: 'none',
													cursor: 'default',
													userSelect: 'none',
												}}
											>
												{position}
											</a>
										)}
									</td>
								</TableRowSignature>
							</tbody>
						</TableSignatureInner>
						<TableSignatureInner style={{ height: 20 }}>
							<tbody>
								<TableRowSignature>
									<td
										className="signature-phone"
										style={{ width: 150, whiteSpace: 'none' }}
									>
										{phoneValue}
									</td>
									<td
										className="signature-website"
										align="center"
										style={{ width: 256, whiteSpace: 'none' }}
									>
										{websiteValue}
									</td>
									<td
										className="signature-media-link-wrapper"
										align="right"
										style={{ width: 122, flexWrap: 'nowrap', paddingLeft: 26 }}
										valign="bottom"
									>
										<table
											style={{
												width: '100%',
											}}
										>
											<tbody>
												<TableRowSignature
													style={{
														textAlign: 'right',
													}}
												>
													{logos.map((logoData) => {
														const { href, alt } = logoData;
														const src =
															mode === 'email' ? logoData.png : logoData.svg;

														return (
															<td
																key={href}
																style={{
																	display: 'inline-flex',
																	height: 16,
																}}
															>
																<a
																	href={href}
																	type="default"
																	className="signature-media-link"
																	target="_blank"
																	rel="noreferrer"
																	style={{
																		display: 'inline-block',
																		height: 16,
																		width: mode === 'default' ? 24 : 20,
																	}}
																>
																	<img
																		src={src}
																		alt={alt}
																		width="16"
																		height="16"
																		style={{
																			width: 16,
																			height: 16,
																		}}
																	/>
																</a>
															</td>
														);
													})}
												</TableRowSignature>
											</tbody>
										</table>
									</td>
								</TableRowSignature>
							</tbody>
						</TableSignatureInner>
					</td>
				</TableRowSignature>
			</tbody>
		</TableSignature>
	);
}
